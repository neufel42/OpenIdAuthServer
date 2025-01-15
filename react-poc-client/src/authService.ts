import { UserManager, User } from 'oidc-client-ts';

const config = {
    authority: 'https://localhost:7003', // Your OpenIddict Authorization Server URL
    client_id: 'react-client',
    redirect_uri: 'http://localhost:3000/callback', // Adjust for Vite's default port
    response_type: 'code',
    scope: 'openid profile api1',
};

const userManager = new UserManager(config);

export const login = (): Promise<void> => userManager.signinRedirect();

export const handleCallback = (): Promise<User> => userManager.signinRedirectCallback();

export const logout = (): Promise<void> => userManager.signoutRedirect();

export const getUser = (): Promise<User | null> => userManager.getUser();
