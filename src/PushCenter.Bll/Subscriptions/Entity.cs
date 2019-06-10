using System.ComponentModel.DataAnnotations;
using JetBrains.Annotations;

namespace PushCenter.Bll.Subscriptions
{
    [UsedImplicitly(ImplicitUseTargetFlags.WithMembers)]
    public class Entity
    {
        [Key]
        public int Id { get; private set; }
    }
}
