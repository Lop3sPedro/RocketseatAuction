using Bogus;
using FluentAssertions;
using Moq;
using RocketseatAuction.API.Contracts;
using RocketseatAuction.API.Entities;
using RocketseatAuction.API.Enums;
using RocketseatAuction.API.UseCases.Auctions.GetCurrent;
using Xunit;

namespace UseCases.Test.Auctions.GetCurrent;

public class GetCurrentAuctionUseCaseTest
{
    [Fact]
    public void Success()
    {
        // ARRANGE -> é onde vai será inicializado tudo que precisa (nesse caso tudo o que precisa nesse useCase);
        var entity = new Faker<Auction>()
            .RuleFor(auction => auction.Id, faker => faker.Random.Number(1, 200))
            .RuleFor(auction => auction.Name, faker => faker.Lorem.Word())
            .RuleFor(auction => auction.Starts, faker => faker.Date.Past())
            .RuleFor(auction => auction.Ends, faker => faker.Date.Future())
            .RuleFor(auction => auction.Items, (faker, auction) => new List<Item>
            {
                new Item
                {
                    Id = faker.Random.Number(1,200),
                    Name = faker.Commerce.ProductName(),
                    Brand = faker.Commerce.Department(),
                    BasePrice = faker.Random.Decimal(50, 1000),
                    Condition = faker.PickRandom<Condition>(),
                    AuctionId = auction.Id,
                }
            }).Generate();

        var mock = new Mock<IAuctionRepository>();
        mock.Setup(i => i.GetCurrent()).Returns(entity);

        var useCase = new GetCurrentAuctionUseCase(mock.Object);

        // ACT -> é a nossa ação (utilizar as instancias que foram criadas);
        var auction = useCase.Execute();

        // ASSERT -> é onde será testado o resultado.
        auction.Should().NotBeNull();
        auction.Id.Should().Be(entity.Id);
        auction.Name.Should().Be(entity.Name);
        auction.Starts.Should().Be(entity.Starts);
        auction.Ends.Should().Be(entity.Ends);
    }
}
