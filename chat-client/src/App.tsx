import AuthPage from "./pages/AuthPage"
import { Routes, Route } from "react-router-dom"
import Welcome from "./pages/Welcome"
import RequireAuth from "./components/RequireAuth"

import { useAppSelector } from "./app/store"
import Layout from "./components/Layout"
import Public from "./components/Public"
import MePage from "./pages/MePage"
import ContactsPage from "./pages/ContactsPage"
import ContactPage from "./pages/ContactPage"
import MessagesPage from "./pages/MessagesPage"
import ConversationPage from "./pages/ConversationPage"
import RegisterPage from "./pages/RegisterPage"

const App = () => {
  return (
    <Routes>
      <Route path="/" element={<Layout />}>
        {/* public routes */}
        <Route path="login" element={<AuthPage />} />
        <Route path="register" element={<RegisterPage />} />

        {/* protected routes */}
        <Route element={<RequireAuth />}>
          <Route path="welcome" element={<Welcome />} />
          <Route path="me" element={<MePage />} />
          <Route path="contacts" element={<ContactsPage />} />
          <Route path="contacts/:id" element={<ContactPage />} />
          <Route path="messages" element={<MessagesPage />} />
          <Route path="messages/:id" element={<ConversationPage />} />
        </Route>
      </Route>
    </Routes>
  )
}

export default App
