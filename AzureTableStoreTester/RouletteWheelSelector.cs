using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureTableStoreTester
{
    public class RouletteWheelSelector
    {
        public class ActionProbability
        {
            public Action Action { get; set; }
            public double Min { get; set; }
            public double Max { get; set; }

            public bool Match(double val)
            {
                return val >= Min && val < Max;
            }
        }

        private readonly IList<ActionProbability> _actions;
        private readonly Random _random;

        private RouletteWheelSelector()
        {
            _actions = new List<ActionProbability>();
            _random = new Random();
        }

        public static RouletteWheelSelector Create(TestParameters parameters)
        {
            RouletteWheelSelector selector = new RouletteWheelSelector();
            double cumulative = 0.0;

            CreateAction(selector._actions, Action.Insert, parameters.InsertProbability, ref cumulative);
            CreateAction(selector._actions, Action.Update, parameters.UpdateProbability, ref cumulative);
            CreateAction(selector._actions, Action.Delete, parameters.DeleteProbability, ref cumulative);
            CreateAction(selector._actions, Action.Retrieve, parameters.RetrieveProbability, ref cumulative);

            return selector;

        }

        public Action? Random()
        {
            double prob = _random.NextDouble();

            foreach (ActionProbability ap in _actions.Where(ap => ap.Match(prob)))
            {
                return ap.Action;
            }

            return null;
        }


        private static void CreateAction(IList<ActionProbability> actions, Action action, double probability, ref double cumulative)
        {
            actions.Add(new ActionProbability
            {
                Action = action,
                Min = cumulative,
                Max = cumulative + probability
            });

            cumulative += probability;
        }

    }
}
