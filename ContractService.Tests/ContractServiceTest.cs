using AutoMapper;
using ContractService.DTOs.Request;
using ContractService.DTOs.Response;
using ContractService.Exceptions;
using ContractService.Messaging.RPC;
using ContractService.Models;
using ContractService.Repositories;
using ContractService.Services;
using Moq;
using Shared.Models;

namespace ContractService.Tests
{
    public class ContractServiceTest
    {
        private readonly Mock<IContractRepository> _contractRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly Mock<CustomerRpcClient> _customerRpcClientMock;
        private readonly IContractService _contractService;

        public ContractServiceTest()
        {
            _contractRepositoryMock = new Mock<IContractRepository>();
            _mapperMock = new Mock<IMapper>();
            _customerRpcClientMock = new Mock<CustomerRpcClient>();

            _contractService = new Services.ContractService(
                _contractRepositoryMock.Object,
                _mapperMock.Object,
                _customerRpcClientMock.Object
            );
        }

        [Fact]
        public async Task CreateContractAsync_Success()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            const double discountRate = 0.1;
            var requestDto = new ContractRequestDto
            {
                CustomerId = customerId,
                DiscountRate = discountRate
            };

            // Dummy customer 
            var dummyCustomer = new CustomerData
            {
                Id = customerId,
                FirstName = "Test",
                LastName = "User",
                VatNumber = "VAT123456",
                Email = "test@example.com",
                CompanyName = "Test Company",
                CreatedAt = DateTime.UtcNow
            };

            // Prepare 
            var contract = new Contract
            {
                CustomerId = customerId,
                DiscountRate = discountRate
            };
            var createdContract = new Contract
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                DiscountRate = discountRate,
                DateIssued = DateTime.UtcNow
            };
            var responseDto = new ContractResponseDto
            {
                Id = createdContract.Id,
                CustomerId = createdContract.CustomerId,
                DiscountRate = createdContract.DiscountRate,
                DateIssued = createdContract.DateIssued
            };

            // Setup mocks 
            _customerRpcClientMock
                .Setup(x => x.GetCustomerAsync(customerId))
                .ReturnsAsync(dummyCustomer);

            _mapperMock
                .Setup(x => x.Map<Contract>(requestDto))
                .Returns(contract);

            _contractRepositoryMock
                .Setup(x => x.CreateContractAsync(contract))
                .ReturnsAsync(createdContract);

            _mapperMock
                .Setup(x => x.Map<ContractResponseDto>(createdContract))
                .Returns(responseDto);

            _contractRepositoryMock
                .Setup(x => x.SaveChangesAsync())
                .Returns(Task.CompletedTask);

            // Act
            var result = await _contractService.CreateContractAsync(requestDto);

            // Assert
            Assert.Equal(responseDto, result);
            _contractRepositoryMock.Verify(x => x.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task CreateContractAsync_CustomerNotFound_ThrowsException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var requestDto = new ContractRequestDto
            {
                CustomerId = customerId,
                DiscountRate = 0.1
            };

            _customerRpcClientMock
                .Setup(x => x.GetCustomerAsync(customerId))
                .ReturnsAsync((CustomerData)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ContractCustomerNotFoundException>(
                () => _contractService.CreateContractAsync(requestDto)
            );
        }

        [Fact]
        public async Task GetLastContractAsync_Success()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            const double discountRate = 0.2;
            var contract = new Contract
            {
                Id = Guid.NewGuid(),
                CustomerId = customerId,
                DiscountRate = discountRate,
                DateIssued = DateTime.UtcNow
            };
            var responseDto = new ContractResponseDto
            {
                Id = contract.Id,
                CustomerId = contract.CustomerId,
                DiscountRate = contract.DiscountRate,
                DateIssued = contract.DateIssued
            };

            _contractRepositoryMock
                .Setup(x => x.GetLastContractAsync(customerId))
                .ReturnsAsync(contract);

            _mapperMock
                .Setup(x => x.Map<ContractResponseDto>(contract))
                .Returns(responseDto);

            // Act
            var result = await _contractService.GetLastContractAsync(customerId);

            // Assert
            Assert.Equal(responseDto, result);
        }

        [Fact]
        public async Task GetLastContractAsync_ContractNotFound_ThrowsException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _contractRepositoryMock
                .Setup(x => x.GetLastContractAsync(customerId))
                .ReturnsAsync((Contract)null!);

            // Act & Assert
            await Assert.ThrowsAsync<ContractNotFoundException>(
                () => _contractService.GetLastContractAsync(customerId)
            );
        }
    }
}