using Alfarid.Shared.Network;
using Xunit;

namespace Alfarid.Tests.Unit;

public sealed class DiscoveryConstantsTests
{
    [Fact]
    public void DiscoveryPort_IsStable()
    {
        Assert.Equal(49555, Discovery.Port);
    }
}
