import React, { FC, useRef, useState, useEffect, RefObject } from "react"
import { Link, useNavigate } from "react-router-dom"

import { useAppDispatch } from "../app/store"
import { setCredentials } from "../features/auth/authSlice"
import { useRegisterMutation } from "../features/auth/authApiSlice"
import { initializeConnection } from "../app/signalRConnection"

interface RegisterPageProps {}

const RegisterPage: FC<RegisterPageProps> = ({}) => {
  const emailRef = useRef() as RefObject<HTMLInputElement>
  const errRef = useRef() as RefObject<HTMLSpanElement>
  const [email, setEmail] = useState("")
  const [password, setPassword] = useState("")
  const [displayName, setDisplayName] = useState("")
  const [errMsg, setErrMsg] = useState("")
  const navigate = useNavigate()

  const [register, { isLoading, error }] = useRegisterMutation()
  const dispatch = useAppDispatch()

  useEffect(() => {
    if (emailRef.current) {
      emailRef.current.focus()
    }
  }, [])

  useEffect(() => {
    setErrMsg("")
  }, [email, password, displayName])

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault()
    if (!email || !password || !displayName) {
      setErrMsg("Please fill out all fields")
      return
    }

    try {
      const result = await register({ email, password, displayName }).unwrap()

      console.log(result)

      dispatch(setCredentials({ ...result, email }))

      setEmail("")
      setPassword("")
      setDisplayName("")

      initializeConnection()

      navigate("/welcome")
    } catch (err: any) {
      console.log(err)
      if (!err?.status) {
        setErrMsg("Network error")
      } else if (err?.status === 400) {
        setErrMsg("Invalid request")
      } else if (err?.status === 409) {
        setErrMsg("Email already in use")
      } else {
        setErrMsg("Unknown error")
      }
      if (errRef.current) {
        errRef.current.focus()
      }
    }
  }

  const handleUserInput = (e: React.ChangeEvent<HTMLInputElement>) =>
    setEmail(e.target.value)

  const handlePasswordInput = (e: React.ChangeEvent<HTMLInputElement>) =>
    setPassword(e.target.value)

  const handleDisplayNameInput = (e: React.ChangeEvent<HTMLInputElement>) =>
    setDisplayName(e.target.value)

  return (
    <div className="min-h-screen flex items-center justify-center bg-gradient-to-br from-black via-gray-900 to-black p-5">
      {isLoading ? (
        <div className="flex items-center justify-center">
          <div className="w-16 h-16 border-4 border-solid rounded-full"></div>
        </div>
      ) : (
        <div className="px-5 rounded-3xl shadow-2xl overflow-hidden relative transform bg-slate-800">
          <div className="absolute inset-0 opacity-25 blur-3xl">
            <div className="h-full"></div>
          </div>
          <div className="px-8 py-12 relative z-10">
            <h2 className="mt-6 text-center text-4xl font-extrabold text-white drop-shadow-md">
              Create an Account
            </h2>
            <p className="mt-2 text-center text-sm text-purple-300 drop-shadow-md">
              Register to get started.
            </p>
          </div>
          <form
            className="mt-8 space-y-6 px-8 pb-8 relative z-10"
            onSubmit={handleSubmit}
          >
            <div>
              <label htmlFor="email-address" className="sr-only">
                Email address
              </label>
              <input
                ref={emailRef}
                id="email-address"
                name="email"
                type="text"
                autoComplete="email"
                required
                className="appearance-none relative block w-full px-3 py-2 border border-purple-700 placeholder-purple-400 text-white bg-transparent rounded-md focus:outline-none focus:ring-purple-500 focus:border-purple-500 focus:z-10 sm:text-sm"
                placeholder="Email address"
                value={email}
                onChange={handleUserInput}
              />
            </div>
            <div>
              <label htmlFor="password" className="sr-only">
                Password
              </label>
              <input
                id="password"
                name="password"
                type="password"
                autoComplete="new-password"
                required
                className="appearance-none relative block w-full px-3 py-2 border border-purple-700 placeholder-purple-400 text-white bg-transparent rounded-md focus:outline-none focus:ring-purple-500 focus:border-purple-500 focus:z-10 sm:text-sm"
                placeholder="Password"
                value={password}
                onChange={handlePasswordInput}
              />
            </div>
            <div>
              <label htmlFor="display-name" className="sr-only">
                Display Name
              </label>
              <input
                id="display-name"
                name="displayName"
                type="text"
                autoComplete="name"
                required
                className="appearance-none relative block w-full px-3 py-2 border border-purple-700 placeholder-purple-400 text-white bg-transparent rounded-md focus:outline-none focus:ring-purple-500 focus:border-purple-500 focus:z-10 sm:text-sm"
                placeholder="Display Name"
                value={displayName}
                onChange={handleDisplayNameInput}
              />
            </div>
            <div>
              <button
                type="submit"
                className="group relative w-full flex justify-center py-2 px-4 border-2 border-transparent text-sm font-medium rounded-md text-white bg-gradient-to-r from-purple-600 to-indigo-600 hover:from-purple-700 hover:to-indigo-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-purple-500 transition-all duration-300 ease-in-out"
              >
                <span className="transition-all duration-300 ease-in-out group-hover:mr-2">
                  Register
                </span>
                <span className="opacity-0 group-hover:opacity-100 transition-all duration-300 ease-in-out">
                  &rarr;
                </span>
              </button>
            </div>
            <div>
              <span ref={errRef} className="text-red-500 drop-shadow-md">
                {errMsg}
              </span>
            </div>
            <div className="text-center">
              <Link
                to="/login"
                className="font-medium text-purple-300 hover:text-white drop-shadow-md transition-colors duration-300 ease-in-out"
              >
                Log In
              </Link>
            </div>
          </form>
        </div>
      )}
    </div>
  )
}

export default RegisterPage
