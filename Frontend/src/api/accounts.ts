import { apiFetch } from './client'
import { createAccountDTO } from './dto/CreateAccountDTO'
import type { Account } from './dto/Account'
import { transferDTO } from './dto/TransferDTO'

export function getAllAccounts(): Promise<Account[]> {
  return apiFetch<Account[]>('/account')
}

export function getAccount(id: number): Promise<Account> {
  return apiFetch<Account>(`/account/${id}`)
}

export function createAccount(balance: number): Promise<Account> {
  return apiFetch<Account>('/account', {
    method: 'POST',
    body: JSON.stringify(createAccountDTO(balance)),
  })
}

export function deleteAccount(id: number): Promise<void> {
  return apiFetch<void>(`/account/${id}`, { method: 'DELETE' })
}

export function transfer( fromAccountId : number, toAccountId : number, amount : number)
{
  return apiFetch<Account>('/account/transfer', {
    method: 'POST',
    body: JSON.stringify(transferDTO(fromAccountId, toAccountId, amount)),
  })
}

