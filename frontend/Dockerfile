FROM --platform=$BUILDPLATFORM node:alpine AS base
FROM base as deps
RUN apk add --no-cache libc6-compat
WORKDIR /app
ARG TARGETARCH
ARG TARGETOS

COPY frontend/package.json frontend/package-lock.json* ./
RUN npm ci --cpu $TARGETARCH --os $TARGETOS

FROM base AS build
WORKDIR /app
COPY --from=deps /app/node_modules ./node_modules
COPY frontend .

ARG NEXT_PUBLIC_OBFUSCATE_IV
ENV NEXT_PUBLIC_OBFUSCATE_IV=${NEXT_PUBLIC_OBFUSCATE_IV}
ARG NEXT_PUBLIC_OBFUSCATE_KEY
ENV NEXT_PUBLIC_OBFUSCATE_KEY=${NEXT_PUBLIC_OBFUSCATE_KEY}
ARG NEXT_PUBLIC_BASEPATH_FILES
ENV NEXT_PUBLIC_BASEPATH_FILES=${NEXT_PUBLIC_BASEPATH_FILES}
ARG NEXT_PUBLIC_BASEPATH_IDENTITY
ENV NEXT_PUBLIC_BASEPATH_IDENTITY=${NEXT_PUBLIC_BASEPATH_IDENTITY}

RUN npm run build

FROM node:alpine AS runner
WORKDIR /app

ENV NODE_ENV production

RUN addgroup --system --gid 1001 nodejs
RUN adduser --system --uid 1001 nextjs

COPY --from=build /app/public ./public

# Set the correct permission for prerender cache
RUN mkdir .next
RUN chown nextjs:nodejs .next

# Automatically leverage output traces to reduce image size
# https://nextjs.org/docs/advanced-features/output-file-tracing
COPY --from=build --chown=nextjs:nodejs /app/.next/standalone ./
COPY --from=build --chown=nextjs:nodejs /app/.next/static ./.next/static

USER nextjs

# set hostname to localhost
ENV HOSTNAME "0.0.0.0"

# server.js is created by next build from the standalone output
# https://nextjs.org/docs/pages/api-reference/next-config-js/output
CMD ["node", "server.js"]