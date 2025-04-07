namespace Cobm.Application.DTOs.Auth;

public record LoginResponseDto(string UserToken, string RefreshToken);