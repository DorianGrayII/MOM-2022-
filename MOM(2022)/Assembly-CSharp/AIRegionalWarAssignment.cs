using System.Collections.Generic;

public class AIRegionalWarAssignment
{
    public SingleRegion owner;

    public List<AIWarTarget> objectives;

    public List<AIWarArmy> armies;

    public AIRegionalWarAssignment(SingleRegion owner)
    {
        this.owner = owner;
        this.objectives = new List<AIWarTarget>();
        this.armies = new List<AIWarArmy>();
    }
}
