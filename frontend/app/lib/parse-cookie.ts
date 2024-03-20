export function parseCookie(cookieString: string): {
    name: string, value: string, httpOnly: boolean, path: string, secure: boolean,
    sameSite: 'strict' | 'lax' | 'none', expires: Date | undefined } | null {
    if (!cookieString) {
        return null; // No cookie string provided
    }

    const cookieParts = cookieString.split('; ');
    if (cookieParts.length === 0) {
        return null; // No valid parts found
    }

    const nameValue = cookieParts[0].split('=');
    if (nameValue.length !== 2) {
        return null; // Invalid cookie format
    }

    const cookieObject: { name: string, value: string, httpOnly: boolean, path: string, secure: boolean,
        sameSite: 'strict' | 'lax' | 'none', expires: Date | undefined } = {
        name: nameValue[0],
        value: nameValue[1],
        httpOnly: false, // Default value, can be overridden
        path: '/', // Default value, can be overridden
        secure: false, // Default value, can be overridden
        sameSite: 'none', // Default value, can be overridden
        expires: undefined, // Default value, can be overridden
    };

    for (let i = 1; i < cookieParts.length; i++) {
        const part = cookieParts[i];
        const [key, value] = part.split('=');

        switch (key.toLowerCase()) {
            case 'httponly':
                cookieObject.httpOnly = true;
                break;
            case 'path':
                cookieObject.path = value;
                break;
            case 'secure':
                cookieObject.secure = true;
                break;
            case 'samesite':
                let x = value.toLowerCase();
                if (x === ('strict' ||'lax' )) {
                    cookieObject.sameSite = x;
                }
                break;
            case 'max-age':
                const maxAge = parseInt(value);
                if (!isNaN(maxAge)) {
                    const expirationDate = new Date(Date.now() + maxAge * 1000);
                    cookieObject.expires = expirationDate;
                }
                break;
        }
    }

    return cookieObject;
}