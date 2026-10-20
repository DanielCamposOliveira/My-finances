export interface Environment {
  production: boolean;
  apiUrl: string;
  googleClientId: string;
}

export const environment: Environment = {
    production: false,
    apiUrl: 'http://localhost:5000/api/v1',
    googleClientId: '208369676612-cc48m14jv1327ae5g37kerii4joeau1p.apps.googleusercontent.com'
};
