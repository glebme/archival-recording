import {createContext, useContext, useEffect, useState} from 'react'
import type {ReactNode} from 'react'
import axios from "axios";

export type UserInfo = {
    subject: string;
    email: string;
    name: string;
    picture: string;
}

type AuthState = {userInfo: UserInfo | null, setUser: (u: UserInfo | null) => void, authLoading: boolean}

const AuthContext = createContext<AuthState | undefined>(undefined)

export const AuthProvider = ({children}: {children: ReactNode}) => {
    const [userInfo, setUser] = useState<UserInfo | null>(null)
    const [authLoading, setAuthLoading] = useState<boolean>(true)

    useEffect(() => {
        axios.get<UserInfo>("/auth/me")
            .then((response) => setUser(response.data))
            .catch(() => setUser(null))
            .finally(() => setAuthLoading(false))
    }, [])

    return (
        <AuthContext.Provider value={{userInfo, setUser, authLoading}}>
            {children}
        </AuthContext.Provider>
    )
}

export const useAuthContext = (): AuthState => {
    const ctx = useContext(AuthContext)
    if (!ctx) throw new Error('useUser must be used within UserProvider')
    return ctx
}
