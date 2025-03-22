using Deployment.Microservice.API.Controllers;
using Deployment.Microservice.APP;
using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Deployment.Microservice.Test
{
    public class DeploymentsControllerTest
    {
       
            private readonly Mock<IDeploymentsServices> _serviceMock;
            private readonly DeploymentsController _controller;

            public DeploymentsControllerTest()
            {
                _serviceMock = new Mock<IDeploymentsServices>();
                _controller = new DeploymentsController(_serviceMock.Object);
            }

            [Fact]
            public async Task NewDeployment_ReturnsOkResult_WhenServiceExecutesSuccessfully()
            {
                // Arrange
                string expectedResponse = "Deployment Successful";
                _serviceMock.Setup(s => s.NewDeployment(It.IsAny<string>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(),
                    It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                    .ReturnsAsync(expectedResponse);

                // Act
                var result = await _controller.NewDeployment("repo", 1, "us-east1", "template", "cluster", "registry", "app", "C#", "cluster", "region", "registry", 1);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                Assert.Equal(expectedResponse, okResult.Value);
            }

            [Fact]
            public async Task TriggerDeploymentGCPDeploy_ReturnsOkResult_WhenServiceExecutesSuccessfully()
            {
                // Arrange
                string repoUrl = "https://github.com/test/repo";
                string expectedResponse = "Trigger Successful";
                _serviceMock.Setup(s => s.TriggerPipelineCommit(repoUrl)).ReturnsAsync(expectedResponse);

                // Act
                var result = await _controller.TriggerDeploymentGCPDeploy(repoUrl);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                Assert.Equal(expectedResponse, okResult.Value);
            }

            [Fact]
            public void AllGCPDeployByID_ReturnsOkResult_WithListOfDeployments()
            {
                // Arrange
                int id = 1;
                var deployments = new List<Deployments> { new Deployments { ID= 1, DEPLOYMENT_NAME = "Test Deployment" } };
                _serviceMock.Setup(s => s.AllDeploymentsById(id)).Returns(deployments);

                // Act
                var result = _controller.AllGCPDeployByID(id);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                Assert.Equal(deployments, okResult.Value);
            }

            [Fact]
            public async Task RemoveDeploymentID_ReturnsOkResult_WhenServiceExecutesSuccessfully()
            {
                // Arrange
                int id = 1;
                string expectedResponse = "Deletion Successful";
                _serviceMock.Setup(s => s.RemoveDeploymentID(id)).ReturnsAsync(expectedResponse);

                // Act
                var result = await _controller.RemoveDeploymentID(id);

                // Assert
                var okResult = Assert.IsType<OkObjectResult>(result.Result);
                Assert.Equal(expectedResponse, okResult.Value);
            }

       


    }
}
