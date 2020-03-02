using System;
using System.Collections.Generic;

namespace CQRSEventSourcing
{
	public class Person
	{
		private int Age { get; set; }
		EventBroker broker;

		public Person(EventBroker broker)
		{
			this.broker = broker;
			broker.Commands += Broker_Commands;
			broker.Queries += Broker_Queries;
		}

		private void Broker_Queries(object sender, Query query)
		{
			var ac = query as AgeQuery;
			if (ac != null && ac.Target == this)
			{
				ac.Result = Age;
			}
		}

		private void Broker_Commands(object sender, Command command)
		{
			var cac = command as ChangeAgeCommand;
			if (cac != null && cac.Target == this)
			{
				Age = cac.Age;
			}
		}
	}

	public class EventBroker
	{
		//1. All events that happened.
		public IList<Event> AllEvents = new List<Event>();
		//2. Commands
		public event EventHandler<Command> Commands;
		//3. Query
		public event EventHandler<Query> Queries;

		public void Command(Command c)
		{
			Commands?.Invoke(this, c);
		}

		public T Query<T>(Query q)
		{
			Queries?.Invoke(this, q);
			return (T) q.Result;
		}
	}

	public class Query
	{
		public object Result;
	}

	public class AgeQuery : Query
	{
		public Person Target;
	}

	public class Command : EventArgs
	{
	}

	public class ChangeAgeCommand : Command
	{
		public Person Target;
		public int Age;

		public ChangeAgeCommand(Person target, int age)
		{
			Target = target;
			Age = age;
		}
	}

	public class Event
	{
	}

	class Program
	{
		static void Main(string[] args)
		{
			var eb = new EventBroker();
			var p = new Person(eb);
			eb.Command(new ChangeAgeCommand(p, 123));

			int age = eb.Query<int>(new AgeQuery { Target = p });
			Console.WriteLine(age);
			Console.ReadLine();
		}
	}
}