import {StrictMode} from 'react'
import {createRoot} from 'react-dom/client'
import './index.css'
import App from './App.tsx'
import {GoogleOAuthProvider} from "@react-oauth/google";
import {BrowserRouter} from "react-router-dom";
import {AuthProvider} from "./contexts/AuthContext.tsx";
import axios from 'axios'

// Configure axios base URL for local development. You can override this
// by setting VITE_API_BASE_URL in your environment (.env.local).
// Example: VITE_API_BASE_URL=https://localhost:57850
const apiBase = (import.meta as any).env?.VITE_API_BASE_URL ?? 'https://localhost:7238'
axios.defaults.baseURL = apiBase
axios.defaults.withCredentials = true

createRoot(document.getElementById('root')!).render(
    <GoogleOAuthProvider clientId={import.meta.env.VITE_GOOGLE_CLIENT_ID}>
        <StrictMode>
            <BrowserRouter>
                <AuthProvider>
                    <App/>
                </AuthProvider>
            </BrowserRouter>
        </StrictMode>
    </GoogleOAuthProvider>,
)
