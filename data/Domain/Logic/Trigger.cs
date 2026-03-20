namespace LowAgeData.Domain.Logic
{
    public class Trigger
    {
        public Trigger(Event @event, IList<Validator>? validators = null)
        {
            Event = @event;
            Validators = validators ?? new List<Validator>();
        }

        /// <summary>
        /// Event to wait for.
        /// </summary>
        public Event Event { get; }
        
        /// <summary>
        /// Used to validate the event before the trigger is activated.
        /// </summary>
        public IList<Validator> Validators { get; }
    }
}
