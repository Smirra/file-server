import crypto from "crypto";

export function obfuscate(plainText: string)
{
    const ivStr = process.env.NEXT_PUBLIC_OBFUSCATE_IV;
    const keyStr = process.env.NEXT_PUBLIC_OBFUSCATE_KEY;
    const iv = Buffer.from(ivStr);
    const key = Buffer.from(keyStr);
    
    const cipher = crypto.createCipheriv('aes-256-cbc', key, iv);
    let obfuscatedText = cipher.update(plainText, 'utf8', 'base64');
    
    obfuscatedText += cipher.final('base64');

    return obfuscatedText;
}