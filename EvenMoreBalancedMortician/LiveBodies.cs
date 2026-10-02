using System.Collections.Generic;
using System.Linq;
using RoR2;

namespace EvenMoreBalancedMortician;

internal static class LiveBodies
{
    public static IEnumerable<CharacterBody> Of(BodyIndex bodyIndex) => bodyIndex == BodyIndex.None
        ? []
        : CharacterBody.readOnlyInstancesList.Where(body => body.bodyIndex == bodyIndex);
}
