using ContractService.DTOs.Request;
using ContractService.Services;
using Microsoft.AspNetCore.Mvc;

namespace ContractService.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ContractController(IContractService contractService) : ControllerBase
{
    [HttpPost("AddContract")]
    public async Task<IActionResult> AddContractAsync(ContractRequestDto contractRequestDto)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var contract = await contractService.CreateContractAsync(contractRequestDto);
        return Created(contract.Id.ToString(), contract);
    }
}