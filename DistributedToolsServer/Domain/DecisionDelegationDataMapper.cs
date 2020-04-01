using System.Linq;

namespace DistributedToolsServer.Domain
{
    public interface IDecisionDelegationDataMapper
    {
        object Map(DecisionDelegationSessionData data);
    }

    public class DecisionDelegationDataMapper : IDecisionDelegationDataMapper
    {
        public object Map(DecisionDelegationSessionData data)
        {
            return new
            {
                data.Users,
                Items = data.Items.Select(i => new
                {
                    i.ItemId,
                    i.Description,
                    i.IsVisible,
                    Votes = i.Votes
                        .Select(v => new
                        {
                            userId = v.Key,
                            vote = v.Value
                        })
                        .ToList()
                }),
                data.CurrentItemId
            };
        }
    }
}
