using TI5yncronizer.Core.ValueObject;

namespace TI5yncronizer.Test.Core.ValueObject;

public class TestPagination
{
    [Fact]
    public void TestMinPageNumberValue()
    {
        Pagination pagination = (pageNumber: -1, limitNumber: 0);

        Assert.Equal(1, pagination.PageNumber);
    }

    [Fact]
    public void TestMinLimitNumberValue()
    {
        Pagination pagination = (pageNumber: 1, limitNumber: -1);

        Assert.Equal(0, pagination.Limit);
    }

    [Fact]
    public void TestMaxLimitNumberValue()
    {
        Pagination pagination = (pageNumber: 1, limitNumber: 150);

        Assert.Equal(100, pagination.Limit);
    }

    [Fact]
    public void TestOffset()
    {
        Pagination pagination = (pageNumber: 1, limitNumber: 0);

        Assert.Equal(0, pagination.Offset);
    }
}
