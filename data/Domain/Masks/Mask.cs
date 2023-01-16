using low_age_data.Domain.Behaviours;
using low_age_data.Domain.Logic;

namespace low_age_data.Domain.Masks
{
    /// <summary>
    /// <see cref="Mask"/> is a bridge between logic components and an area in which they should be applied. It can
    /// be used to create areas in which certain triggers need to happen. <see cref="MaskProvider"/> creates the
    /// <see cref="Mask"/>, while usage of <see cref="Event.EntityMaskChanged"/> or <see cref="MaskCondition"/> applies
    /// logic.
    /// 
    /// More specifically, it is used for Power field in which some entities generate the Power, while other entities
    /// require it to function, otherwise the abilities are disabled, etc.
    /// </summary>
    public class Mask
    {
        public Mask(
            MaskName name)
        {
            Name = name;
        }
        
        public MaskName Name { get; }
    }
}