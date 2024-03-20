/** @type {import('next').NextConfig} */
const dotenv = require('dotenv')
const dotenvExpand = require('dotenv-expand')

dotenvExpand.expand(dotenv.config({ path: `../.env.dev` }))
dotenvExpand.expand(dotenv.config({ path: `../.env` }))

const nextConfig = {
  output: 'standalone',
}

module.exports = nextConfig