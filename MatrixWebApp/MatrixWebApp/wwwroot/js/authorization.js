export function getToken() {
    return localStorage.getItem('jwtToken');
}

export function setToken(token) {
    localStorage.setItem('jwtToken', token);
}

export function clearToken() {
    localStorage.removeItem('jwtToken');
}

export async function authorizedFetch(url, options = {}) {
    const token = getToken();

    const headers = options.headers ? new Headers(options.headers) : new Headers();

    if (token) {
        headers.set('Authorization', `Bearer ${token}`);
    }

    const opts = {
        ...options,
        headers
    };

    const response = await fetch(url, opts);

    return response;
}

import { authorizedFetch } from './authorization.js';

async function getProtectedData() {
    const response = await authorizedFetch('/api/protected');
    const data = await response.json();
    return data;
}