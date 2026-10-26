# ADR-0013: Autoryzacja panelu admina przez JWT

- **Status:** Accepted
- **Data:** 2026-10-21
- **Powiązane:** ADR-0012

## Kontekst
Dashboard ma być chroniony hasłem. Klienci sklepu nie mają kont (zakup bez logowania).

## Decyzja
- Jedno konto administratora tworzone przy starcie z `ADMIN_EMAIL` i `ADMIN_PASSWORD_HASH` (zmienne środowiskowe).
  Hasło hashowane `PasswordHasher<T>` z ASP.NET Core.
- `POST /api/admin/login` zwraca JWT (ważność 8 h, podpis HS256 kluczem z `JWT_SECRET`), rola `Admin`.
- Endpointy `/api/admin/**` z `[Authorize(Roles = "Admin")]`.
- Angular: token w pamięci serwisu + `sessionStorage`, `HttpInterceptor` dokleja nagłówek, `CanMatch` guard na `/admin`.
- Rate limit na logowanie: 5 prób / min / IP.

## Rozważane alternatywy
- **ASP.NET Core Identity z rejestracją** — za dużo infrastruktury na jedno konto.
- **Basic Auth** — prostsze, ale słabiej wygląda w pracy i gorzej współpracuje z SPA.

## Konsekwencje
- Pozytywne: mało kodu, standardowe podejście.
- Negatywne: brak odświeżania tokenu i wylogowania po stronie serwera (akceptowalne w projekcie).
