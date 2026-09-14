using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BankDataAPI.Models;
using BankDataAPI.DTO;
using BankDataAPI.Services;

namespace BankDataAPI.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class AccountController : ControllerBase
{
    private AccountContext _context;
    private IAccountService _accountService;

    private bool TryGetUserId(out Guid userId)
    {
        var claim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value 
            ?? User.FindFirst("sub")?.Value;

        return Guid.TryParse(claim, out userId);
    }

    public AccountController(AccountContext context, IAccountService accountService)
    {
        _context = context;
        _accountService = accountService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<Account>>> GetAll()
    {

        // This should be wrapped somehow
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid or missing user ID in token.");

        var accounts = await _context.Accounts
            .Where(a => a.UserId == userId)
            .ToListAsync();

        return Ok(accounts);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<Account>> Get(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid or missing user ID in token.");

        var account = await _context.Accounts
            .Where(a => a.UserId == userId && a.Id == id)
            .FirstOrDefaultAsync();

        if (account == null)
            return NotFound();

        return Ok(account);
    }

    [HttpPost]
    public async Task<ActionResult<Account>> Post(CreateAccountDTO dto)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid or missing user ID in token.");

        Account account =  new Account
        {
            Id = 0,
            Balance = dto.Balance,
            UserId = userId
        };

        _context.Accounts.Add(account);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(Get), new { id = account.Id }, account);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid or missing user ID in token.");

        var account = await _context.Accounts
            .Where(a => a.UserId == userId && a.Id == id)
            .FirstOrDefaultAsync();

        if (account == null)
            return NotFound();

        _context.Accounts.Remove(account);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("transfer")]
    public async Task<IActionResult> transfer(TransferDTO transferDTO)
    {
        if (!TryGetUserId(out var userId))
            return Unauthorized("Invalid or missing user ID in token.");

        try {
            var result = await _accountService.TransferAsync(
                transferDTO.FromAccountId, 
                transferDTO.ToAccountId, 
                transferDTO.Amount,
                userId
            );
            return Ok(result);

        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
        catch (ArgumentException ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
