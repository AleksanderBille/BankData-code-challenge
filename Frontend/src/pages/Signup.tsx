import { useState } from 'react'
import { useNavigate, Link } from 'react-router-dom'
import { supabase } from '../supabaseClient'

export default function Signup() {
  const [email, setEmail] = useState('')
  const [password, setPassword] = useState('')
  const [error, setError] = useState(null)
  const [success, setSuccess] = useState(false)
  const navigate = useNavigate()

  const handleSignup = async (e) => {
    e.preventDefault()
    setError(null)

    const { error } = await supabase.auth.signUp({ email, password })

    if (error) {
      setError(error.message)
      return
    }

    setSuccess(true)
    // If you disabled email confirmation, the user is already logged in at this point
    navigate('/')
  }

  return (
    <form onSubmit={handleSignup}>
      <h1>Sign up</h1>
      {error && <p style={{ color: 'red' }}>{error}</p>}
      {success && <p>Check your email to confirm your account.</p>}
      <input
        type="email"
        placeholder="Email"
        value={email}
        onChange={(e) => setEmail(e.target.value)}
        required
      />
      <input
        type="password"
        placeholder="Password"
        value={password}
        onChange={(e) => setPassword(e.target.value)}
        required
      />
      <button type="submit">Sign up</button>
      <p>Already have an account? <Link to="/login">Log in</Link></p>
    </form>
  )
}