namespace LowAgeData.Domain.Logic
{
    public class Trigger
    {
        public Trigger(IList<Event> events, IList<Validator>? validators = null)
        {
            Events = events;
            Validators = validators ?? new List<Validator>();
        }

        public IList<Event> Events { get; } // Events to wait for (logical AND between the events
                                            // for the trigger to be considered fired)
        public IList<Validator> Validators { get; } // More options to validate the events
    }
}
