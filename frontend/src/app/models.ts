// The complete GitHub repository object from the search API. Only the identifying fields are typed:
// `id` keys the bookmark, and the name and owner avatar are shown on the card. Every other GitHub field
// is kept through the index signature and sent unchanged to the bookmark session.
export interface Repo {
  id: number;
  full_name: string;
  owner: { login: string; avatar_url: string };
  [key: string]: unknown;
}

export interface LoginResponse {
  token: string;
  username: string;
  expiresAt: string;
}
