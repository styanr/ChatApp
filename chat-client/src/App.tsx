import AuthPage from "./pages/AuthPage"
import { Routes, Route } from "react-router-dom"
import Welcome from "./pages/Welcome"
import RequireAuth from "./components/RequireAuth"

import { useAppSelector } from "./app/store"
import Layout from "./components/Layout"
import Public from "./components/Public"

const App = () => {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        {/* public routes */}
        <Route index element={<Public />} />
        <Route path="login" element={<AuthPage />} />

        {/* protected routes */}
        <Route element={<RequireAuth />}>
          <Route path="welcome" element={<Welcome />} />
        </Route>
        
      </Route>
    </Routes>
  )
}

export default App
