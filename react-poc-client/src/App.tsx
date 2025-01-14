import React, { useEffect, useState } from 'react';
import { login, handleCallback, getUser, logout } from './authService';

const App: React.FC = () => {
    const [user, setUser] = useState<any>(null);

    useEffect(() => {
        // Handle the redirect callback and get the user data
        handleCallback()
            .then(() => getUser().then(setUser))
            .catch(() => login());
    }, []);

    return (
        <div>
            <h1>React OpenIddict POC with Vite and TypeScript</h1>
            {!user ? (
                <button onClick={login}>Login</button>
            ) : (
                <div>
                    <p>Welcome, {user?.profile?.name}!</p>
                    <button onClick={() => logout()}>Logout</button>
                </div>
            )}
        </div>
    );
};

export default App;