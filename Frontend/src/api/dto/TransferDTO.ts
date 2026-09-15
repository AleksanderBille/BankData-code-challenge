export interface TransferDTO {
  fromAccountId : number
  toAccountId : number
  amount : number
}

export function transferDTO(  
  fromAccountId : number,
  toAccountId : number,
  amount : number
): TransferDTO {
  return { 
    fromAccountId, 
    toAccountId, 
    amount 
  }
}