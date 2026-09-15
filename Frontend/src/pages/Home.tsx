import { useState, useEffect } from 'react'
import { supabase } from '../supabaseClient'
import { useNavigate } from 'react-router-dom'
import { getAllAccounts, createAccount, transfer } from '../api/accounts'
import type { Account } from '../api/dto/Account'

export default function Home() {
  const [balance, setBalance] = useState('')
  const [error, setError] = useState<string | null>(null)
  const [accounts, setAccounts] = useState<Account[]>([])

  const [fromAccountId, setFromAccountId] = useState(0)
  const [toAccountId, setToAccountId] = useState(0)
  const [amount, setAmount] = useState(0)

  const navigate = useNavigate()

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)

    try {
      const account = await createAccount(parseFloat(balance))
      setBalance('')
      setAccounts((prev) => [...prev, account])
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error')
    }
  }

  const handleLogout = async () => {
    await supabase.auth.signOut()
    navigate('/login')
  }

  const handleTransfer = async (e: React.FormEvent) => {
    e.preventDefault()
    setError(null)

    try {
      await transfer(fromAccountId, toAccountId, amount)
      setFromAccountId(0)
      setToAccountId(0)
      setAmount(0)
      await loadAccounts() // refresh balances after a successful transfer
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error')
    }
  }

  const loadAccounts = async () => {
    try {
      const data = await getAllAccounts()
      setAccounts(data || [])
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Unknown error')
    }
  }

  useEffect(() => {
    loadAccounts()
  }, [])

  return (
    <div>
      <div>
        <form onSubmit={handleSubmit}>
          {error && <p style={{ color: 'red' }}>{error}</p>}
          <input
            type="number"
            step="0.01"
            placeholder="Initial balance"
            value={balance}
            onChange={(e) => setBalance(e.target.value)}
            required
          />
          <button type="submit">Create account</button>
        </form>
      </div>

      <div>
        <h2>Your accounts</h2>
        <ul>
          {accounts.map((account) => (
            <li key={account.id}>ID: #{account.id} — Balance: {account.balance}</li>
          ))}
        </ul>
      </div>

      <div>
        <h1> TRANSFER </h1>
        <form onSubmit={handleTransfer}>
          <p>From account ID</p>
          <input
            type="number"
            value={fromAccountId}
            onChange={(e) => setFromAccountId(Number(e.target.value))}
            required
          />
          <p>To account ID</p>
          <input
            type="number"
            value={toAccountId}
            onChange={(e) => setToAccountId(Number(e.target.value))}
            required
          />
          <p>Amount</p>
          <input
            type="number"
            value={amount}
            onChange={(e) => setAmount(Number(e.target.value))}
            required
          />
          <button type="submit">Transfer</button>
        </form>
      </div>

      <div>
        <p>You're logged in.</p>
        <button onClick={handleLogout}>Log out</button>
      </div>
    </div>
  )
}