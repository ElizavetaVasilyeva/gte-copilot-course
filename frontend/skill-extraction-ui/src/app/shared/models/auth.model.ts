export interface AuthToken {
  token: string;
  expiresInSeconds: number;
}

export interface SignUpRequest {
  username: string;
  password: string;
}

export interface SignInRequest {
  username: string;
  password: string;
}
