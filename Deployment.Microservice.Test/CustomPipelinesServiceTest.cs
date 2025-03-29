using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;
using Microsoft.AspNetCore.Mvc;
using Deployment.Microservice.APP;
using Deployment.Microservice.Domain;

namespace Deployment.Microservice.Tests
{
    public class CustomPipelinesServicesTests
    {
        private readonly Mock<ICustomerPipelinesRepository> _repositoryMock;
        private readonly CustomPipelinesServices _service;

        public CustomPipelinesServicesTests()
        {
            _repositoryMock = new Mock<ICustomerPipelinesRepository>();
            _service = new CustomPipelinesServices(_repositoryMock.Object);
        }

        [Fact]
        public async Task UpdatePipeline_ReturnsFileContentResult_WhenValidInput()
        {
            var templateContent = "pipeline content with {{CUSTOMER_ID}} and {{APP_NAME}}";
            var fileContent = new FileContentResult(Encoding.UTF8.GetBytes(templateContent), "text/yaml");
            _repositoryMock.Setup(r => r.GetPipelineTemplate(It.IsAny<int>(), "build")).ReturnsAsync(fileContent);

            var result = await _service.UpdatePipeline(1, 1, "cluster", "artifact", "us-east1", "myapp");

            Assert.IsType<FileContentResult>(result);
            var content = Encoding.UTF8.GetString(result.FileContents);
            Assert.Contains("1", content);
            Assert.Contains("myapp", content);
        }

        [Fact]
        public async Task UpdatePipeline_ThrowsArgumentException_WhenClusterNameOrAppNameIsEmpty()
        {
            await Assert.ThrowsAsync<ArgumentException>(() => _service.UpdatePipeline(1, 1, "", "artifact", "us-east1", "myapp"));
        }

        [Fact]
        public async Task UpdatePipeline_ThrowsFileNotFoundException_WhenFileContentsNull()
        {
            _repositoryMock.Setup(r => r.GetPipelineTemplate(It.IsAny<int>(), "build")).ReturnsAsync((FileContentResult)null);
            await Assert.ThrowsAsync<FileNotFoundException>(() => _service.UpdatePipeline(1, 1, "cluster", "artifact", "us-east1", "myapp"));
        }

        [Fact]
        public async Task DropGitHub_ReturnsListOfObjects()
        {
            var list = new List<object> { "repo1", "repo2" };
            _repositoryMock.Setup(r => r.dropGitHub()).ReturnsAsync(list);

            var result = await _service.dropGitHub();

            Assert.Equal(list, result);
        }

        [Fact]
        public async Task DropSecrets_ReturnsListOfObjects()
        {
            var list = new List<object> { "secret1", "secret2" };
            _repositoryMock.Setup(r => r.dropSecrets()).ReturnsAsync(list);

            var result = await _service.dropSecrets();

            Assert.Equal(list, result);
        }
    }
}
