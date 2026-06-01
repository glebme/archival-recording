import './App.css'
import LoginPage from './pages/LoginPage.tsx'
import DashboardPage from './pages/DashboardPage.tsx'
import ProtectedRoute from './components/ProtectedRoute.tsx'
import {Navigate, Route, Routes} from "react-router-dom";

function App() {
  return (
      <Routes>
        <Route path="/login" element={<LoginPage/>}/>
          <Route element={<ProtectedRoute/>}>
              <Route path="/dashboard" element={<DashboardPage/>}/>
          </Route>
        <Route path={"*"} element={<Navigate to="/login" replace/>}/>
      </Routes>
  )
}

export default App
