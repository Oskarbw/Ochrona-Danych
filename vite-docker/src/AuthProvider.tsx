import React, { ReactNode, useState, createContext } from 'react'
type AuthProviderPropsType = {
    children : ReactNode
}

type ContextType = {
    getUser: () => (UserType | null),
    login : (user : UserType) => void,
    logout : () => void
}
export type UserType= {
    username? : string | null
    token? : string | null
    accountNumber? : string | null,
}

export const AuthContext = createContext<ContextType>({
    getUser : () => null,
    login : () => undefined,
    logout : () => undefined,
});

const AuthProvider = ({ children } : AuthProviderPropsType) => {
    const [user, setUser] = useState<UserType | null>(null);
    
    const getUser = () => {
        const userFromLocal = localStorage.getItem('user')
        if(userFromLocal != null) {
            return JSON.parse(userFromLocal)
        }
        return null
    }

    const login = (user : UserType) => {
        if(user != null) {
            localStorage.setItem('user', JSON.stringify(user));
            setUser(user);
        }
    }

    const logout = () => {
        localStorage.removeItem('user');
        setUser(null);
    }
    return (
        <AuthContext.Provider value= {{ getUser, login, logout }}>
            { children }
        </AuthContext.Provider>
    )
}

export default AuthProvider