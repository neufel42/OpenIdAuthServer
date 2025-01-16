import { UserManager, User } from 'oidc-client-ts';

const config = {
    authority: 'https://localhost:7003', // Your OpenIddict Authorization Server URL
    client_id: 'react-client',
    client_secret: 'react-secret',
    redirect_uri: 'http://localhost:3000/callback', // Adjust for Vite's default port
    response_type: 'code',
    scope: 'profile email roles address',
};

console.log("AUTH", config);

const userManager = new UserManager(config);

export const login = (): Promise<void> => userManager.signinRedirect();

//export const handleCallback = (): Promise<User> => userManager.signinRedirectCallback();
export const handleCallback = async (): Promise<User> => {
    try {
      const user = await userManager.signinRedirectCallback();
      console.log("USER:", user);
      return user;  // Return the user object after successful sign-in
    } catch (error) {
      console.error('Error handling callback:', error);
      throw new Error('Failed to handle callback');
    }
  };

export const logout = (): Promise<void> => userManager.signoutRedirect();

export const getUser = (): Promise<User | null> => userManager.getUser();
