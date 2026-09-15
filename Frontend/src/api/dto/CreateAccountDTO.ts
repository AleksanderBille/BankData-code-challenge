export interface CreateAccountDTO {
  balance: number
}

export function createAccountDTO(balance: number): CreateAccountDTO {
  return { balance }
}