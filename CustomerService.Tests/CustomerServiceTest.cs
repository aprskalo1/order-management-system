using AutoMapper;
using CustomerService.DTOs.Request;
using CustomerService.DTOs.Response;
using CustomerService.Exceptions;
using CustomerService.Models;
using CustomerService.Repositories;
using CustomerService.Services;
using Moq;

namespace CustomerService.Tests
{
    public class CustomerServiceTest
    {
        private readonly Mock<ICustomerRepository> _repositoryMock;
        private readonly Mock<IMapper> _mapperMock;
        private readonly ICustomerService _customerService;

        public CustomerServiceTest()
        {
            _repositoryMock = new Mock<ICustomerRepository>();
            _mapperMock = new Mock<IMapper>();

            _customerService = new Services.CustomerService(
                _repositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task CreateCustomerAsync_Success()
        {
            // Arrange
            var requestDto = new CustomerRequestDto
            {
                FirstName = "John",
                LastName = "Doe",
                Email = "john@example.com",
                VatNumber = "VAT123",
                CompanyName = "John's Company"
            };

            var customer = new Customer
            {
                Id = Guid.NewGuid(),
                FirstName = requestDto.FirstName,
                LastName = requestDto.LastName,
                Email = requestDto.Email,
                VatNumber = requestDto.VatNumber,
                CompanyName = requestDto.CompanyName,
                CreatedAt = DateTime.UtcNow
            };

            var responseDto = new CustomerResponseDto
            {
                Id = customer.Id,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                VatNumber = customer.VatNumber,
                CompanyName = customer.CompanyName,
                CreatedAt = customer.CreatedAt
            };

            _mapperMock.Setup(m => m.Map<Customer>(requestDto)).Returns(customer);
            _repositoryMock.Setup(r => r.CreateCustomerAsync(customer)).ReturnsAsync(customer);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<CustomerResponseDto>(customer)).Returns(responseDto);

            // Act
            var result = await _customerService.CreateCustomerAsync(requestDto);

            // Assert
            Assert.Equal(responseDto.Id, result.Id);
            Assert.Equal(responseDto.FirstName, result.FirstName);
            Assert.Equal(responseDto.LastName, result.LastName);
            _repositoryMock.Verify(r => r.CreateCustomerAsync(customer), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_Success()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var customer = new Customer
            {
                Id = customerId,
                FirstName = "Jane",
                LastName = "Doe",
                Email = "jane@example.com",
                CreatedAt = DateTime.UtcNow
            };
            var responseDto = new CustomerResponseDto
            {
                Id = customerId,
                FirstName = customer.FirstName,
                LastName = customer.LastName,
                Email = customer.Email,
                VatNumber = customer.VatNumber,
                CompanyName = customer.CompanyName,
                CreatedAt = customer.CreatedAt
            };

            _repositoryMock.Setup(r => r.GetCustomerByIdAsync(customerId)).ReturnsAsync(customer);
            _mapperMock.Setup(m => m.Map<CustomerResponseDto>(customer)).Returns(responseDto);

            // Act
            var result = await _customerService.GetCustomerByIdAsync(customerId);

            // Assert
            Assert.Equal(responseDto.Id, result.Id);
            Assert.Equal(responseDto.FirstName, result.FirstName);
            Assert.Equal(responseDto.LastName, result.LastName);
        }

        [Fact]
        public async Task GetCustomerByIdAsync_NotFound_ThrowsException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetCustomerByIdAsync(customerId)).ReturnsAsync((Customer)null!);

            // Act & Assert
            var ex = await Assert.ThrowsAsync<CustomerNotFoundException>(
                () => _customerService.GetCustomerByIdAsync(customerId)
            );
            Assert.Contains(customerId.ToString(), ex.Message);
        }

        [Fact]
        public async Task GetCustomersAsync_Success()
        {
            // Arrange
            var customers = new List<Customer>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    FirstName = "Alice",
                    LastName = "Smith",
                    Email = "alice@example.com",
                    CreatedAt = DateTime.UtcNow
                }
            };
            var responseDtos = customers.Select(c => new CustomerResponseDto
            {
                Id = c.Id,
                FirstName = c.FirstName,
                LastName = c.LastName,
                Email = c.Email,
                VatNumber = c.VatNumber,
                CompanyName = c.CompanyName,
                CreatedAt = c.CreatedAt
            }).ToList();

            _repositoryMock.Setup(r => r.GetCustomersAsync()).ReturnsAsync(customers);
            _mapperMock.Setup(m => m.Map<IEnumerable<CustomerResponseDto>>(customers)).Returns(responseDtos);

            // Act
            var result = await _customerService.GetCustomersAsync();

            // Assert
            Assert.Equal(responseDtos.Count, result.Count());
        }

        [Fact]
        public async Task GetCustomersAsync_NoCustomers_ThrowsException()
        {
            // Arrange
            _repositoryMock.Setup(r => r.GetCustomersAsync()).ReturnsAsync(new List<Customer>());

            // Act & Assert
            await Assert.ThrowsAsync<CustomerNotFoundException>(() => _customerService.GetCustomersAsync());
        }

        [Fact]
        public async Task UpdateCustomerAsync_Success()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var existingCustomer = new Customer
            {
                Id = customerId,
                FirstName = "Bob",
                LastName = "Brown",
                Email = "bob@example.com",
                CreatedAt = DateTime.UtcNow
            };
            var requestDto = new CustomerRequestDto
            {
                FirstName = "Robert",
                LastName = "Brown",
                Email = "robert@example.com",
                VatNumber = "VAT987",
                CompanyName = "Robert Co."
            };
            var updatedResponseDto = new CustomerResponseDto
            {
                Id = customerId,
                FirstName = requestDto.FirstName,
                LastName = requestDto.LastName,
                Email = requestDto.Email,
                VatNumber = requestDto.VatNumber,
                CompanyName = requestDto.CompanyName,
                CreatedAt = existingCustomer.CreatedAt
            };

            _repositoryMock.Setup(r => r.GetCustomerByIdAsync(customerId)).ReturnsAsync(existingCustomer);
            _mapperMock.Setup(m => m.Map(requestDto, existingCustomer)).Callback(() =>
            {
                existingCustomer.FirstName = requestDto.FirstName;
                existingCustomer.LastName = requestDto.LastName;
                existingCustomer.Email = requestDto.Email;
                existingCustomer.VatNumber = requestDto.VatNumber;
                existingCustomer.CompanyName = requestDto.CompanyName;
            });
            _repositoryMock.Setup(r => r.UpdateCustomerAsync(existingCustomer)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);
            _mapperMock.Setup(m => m.Map<CustomerResponseDto>(existingCustomer)).Returns(updatedResponseDto);

            // Act
            var result = await _customerService.UpdateCustomerAsync(requestDto, customerId);

            // Assert
            Assert.Equal(updatedResponseDto.FirstName, result.FirstName);
            Assert.Equal(updatedResponseDto.LastName, result.LastName);
            _repositoryMock.Verify(r => r.UpdateCustomerAsync(existingCustomer), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task UpdateCustomerAsync_NotFound_ThrowsException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var requestDto = new CustomerRequestDto
            {
                FirstName = "No",
                LastName = "Body",
                Email = "nobody@example.com"
            };

            _repositoryMock.Setup(r => r.GetCustomerByIdAsync(customerId)).ReturnsAsync((Customer)null!);

            // Act & Assert
            await Assert.ThrowsAsync<CustomerNotFoundException>(
                () => _customerService.UpdateCustomerAsync(requestDto, customerId)
            );
        }

        [Fact]
        public async Task DeleteCustomerAsync_Success()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            var existingCustomer = new Customer
            {
                Id = customerId,
                FirstName = "Delete",
                LastName = "Me",
                Email = "delete@example.com",
                CreatedAt = DateTime.UtcNow
            };

            _repositoryMock.Setup(r => r.GetCustomerByIdAsync(customerId)).ReturnsAsync(existingCustomer);
            _repositoryMock.Setup(r => r.DeleteCustomerAsync(existingCustomer)).Returns(Task.CompletedTask);
            _repositoryMock.Setup(r => r.SaveChangesAsync()).Returns(Task.CompletedTask);

            // Act
            await _customerService.DeleteCustomerAsync(customerId);

            // Assert
            _repositoryMock.Verify(r => r.DeleteCustomerAsync(existingCustomer), Times.Once);
            _repositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
        }

        [Fact]
        public async Task DeleteCustomerAsync_NotFound_ThrowsException()
        {
            // Arrange
            var customerId = Guid.NewGuid();
            _repositoryMock.Setup(r => r.GetCustomerByIdAsync(customerId)).ReturnsAsync((Customer)null!);

            // Act & Assert
            await Assert.ThrowsAsync<CustomerNotFoundException>(
                () => _customerService.DeleteCustomerAsync(customerId)
            );
        }
    }
}