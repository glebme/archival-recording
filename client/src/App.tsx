import './App.css'
import LoginPage from './pages/LoginPage.tsx'
import {Navigate, Route, Routes} from "react-router-dom";

function App() {
  return (
      <Routes>
        <Route path="/login" element={<LoginPage/>}/>
        <Route path={"*"} element={<Navigate to="/login" replace/>}/>
      </Routes>
  )
}

export default App
