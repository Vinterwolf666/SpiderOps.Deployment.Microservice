using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Deployment.Microservice.APP;
using Infrastructure.Microservice.APP;
using Infrastructure.Microservice.Domain;


public class GCPInfrastructureServicesTests
{
    private readonly Mock<IGCPInfrastructureRepository> _repositoryMock;
    private readonly GCPInfrastructureServices _service;

    public GCPInfrastructureServicesTests()
    {
        _repositoryMock = new Mock<IGCPInfrastructureRepository>();
        _service = new GCPInfrastructureServices(_repositoryMock.Object);
    }

    [Fact]
    public void AllInfrastructureByID_ReturnsListOfInfrastructure()
    {
        int customerId = 1;
        var expected = new List<GCPInfrastructure> { new GCPInfrastructure { CUSTOMER_ID = customerId } };

        _repositoryMock.Setup(repo => repo.AllInfrastructureByID(customerId)).Returns(expected);

        var result = _service.AllInfrastructureByID(customerId);

        Assert.Equal(expected, result);
    }

   

    [Fact]
    public async Task RemoveNewInfrastructure_ReturnsSuccessMessage()
    {
        int customerId = 1;
        string expectedMessage = "Infrastructure removed successfully.";

        _repositoryMock.Setup(repo => repo.RemoveInfrastructureByID(customerId)).ReturnsAsync(expectedMessage);

        var result = await _service.RemoveNewInfrastructure(customerId);

        Assert.Equal(expectedMessage, result);
    }
}
