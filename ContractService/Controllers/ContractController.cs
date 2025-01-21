using ContractService.DTOs.Request;
using ContractService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContractService.Controllers;

[ApiController]
[Route("api/contracts")]
public class ContractController(IContractService contractService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddContractAsync(ContractRequestDto contractRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var contract = await contractService.CreateContractAsync(contractRequestDto);
        return Created($"/api/contracts/{contract.Id}", contract);
    }

    [HttpGet("last/{customerId:guid}")]
    public async Task<IActionResult> GetLastContractAsync(Guid customerId)
    {
        var contract = await contractService.GetLastContractAsync(customerId);
        return Ok(contract);
    }
}