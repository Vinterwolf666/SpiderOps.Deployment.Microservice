using Deployment.Microservice.APP;
using Deployment.Microservice.Domain;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Xunit;

public class PipelinesServicesTests
{
    private readonly Mock<IPipelinesRepository> _repositoryMock;
    private readonly PipelinesServices _service;

    public PipelinesServicesTests()
    {
        _repositoryMock = new Mock<IPipelinesRepository>();
        _service = new PipelinesServices(_repositoryMock.Object);
    }

    [Fact]
    public void AllPipelinesByID_ReturnsPipelinesList()
    {
        int customerId = 1;
        var pipelines = new List<Pipelines> { new Pipelines() };

        _repositoryMock.Setup(repo => repo.AllPipelinesByID(customerId)).Returns(pipelines);

        var result = _service.AllPipelinesByID(customerId);

        Assert.Equal(pipelines, result);
    }

    [Fact]
    public async Task DownloadPipeline_ReturnsFileContentResult()
    {
        int pipelineId = 1;
        string fileName = "pipeline.yaml";
        var fileResult = new FileContentResult(new byte[] { 1, 2, 3 }, "text/yaml");

        _repositoryMock.Setup(repo => repo.DownloadPipeline(pipelineId, fileName)).ReturnsAsync(fileResult);

        var result = await _service.DownloadPipeline(pipelineId, fileName);

        Assert.Equal(fileResult, result);
    }

    [Fact]
    public async Task DownloadPipeline_FileNotFound_ThrowsException()
    {
        int pipelineId = 1;
        string fileName = "missing.yaml";

        _repositoryMock.Setup(repo => repo.DownloadPipeline(pipelineId, fileName)).ThrowsAsync(new FileNotFoundException());

        await Assert.ThrowsAsync<Exception>(async () => await _service.DownloadPipeline(pipelineId, fileName));
    }

    [Fact]
    public async Task RemovePipelinesByID_ReturnsSuccessMessage()
    {
        int pipelineId = 1;
        string expectedMessage = "Pipeline removed.";

        _repositoryMock.Setup(repo => repo.RemovePipelinesByID(pipelineId)).ReturnsAsync(expectedMessage);

        var result = await _service.RemovePipelinesByID(pipelineId);

        Assert.Equal(expectedMessage, result);
    }

    [Fact]
    public async Task UploadPipeline_ValidFile_ReturnsSuccessMessage()
    {
        int customerId = 1;
        var yamlFile = new Mock<IFormFile>();
        yamlFile.Setup(f => f.Length).Returns(1);
        string expectedMessage = "Pipeline uploaded.";

        _repositoryMock.Setup(repo => repo.UploadPipeline(customerId, yamlFile.Object, "cloud", "C#", "desc", "app", "cluster", "artifact", "region")).ReturnsAsync(expectedMessage);

        var result = await _service.UploadPipeline(customerId, yamlFile.Object, "cloud", "C#", "desc", "app", "cluster", "artifact", "region");

        Assert.Equal(expectedMessage, result);
    }

    [Fact]
    public async Task UploadPipeline_InvalidFile_ReturnsErrorMessage()
    {
        int customerId = 1;
        var yamlFile = new Mock<IFormFile>();
        yamlFile.Setup(f => f.Length).Returns(0);

        var result = await _service.UploadPipeline(customerId, yamlFile.Object, "cloud", "C#", "desc", "app", "cluster", "artifact", "region");

        Assert.Equal("Invalid YAML file.", result);
    }
}
