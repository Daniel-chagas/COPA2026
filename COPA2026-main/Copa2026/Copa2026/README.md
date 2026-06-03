# Sistema Copa do Mundo 2026 — C# Console

Projeto prático de programação estruturada em C#.

## Como compilar e executar

### Pré-requisito
.NET 8 SDK instalado → https://dotnet.microsoft.com/download

### Passos
```bash
# Na pasta raiz do projeto (onde está Copa2026.csproj)
dotnet build
dotnet run
```

Ou em modo Release:
```bash
dotnet run --configuration Release
```

---

## Estrutura do projeto

```
Copa2026/
├── Copa2026.csproj
├── README.md
├── csv/                        ← Arquivos CSV gerados e carregados
│   ├── selecoes.csv            (48 seleções pré-cadastradas)
│   ├── estadios.csv            (16 estádios da Copa 2026)
│   ├── jogos.csv
│   ├── classificacao.csv
│   ├── mata_mata.csv
│   └── relatorio_final.csv
└── src/
    ├── Program.cs              ← Ponto de entrada
    ├── Dados.cs                ← Structs + vetores + matriz globais
    ├── UI.cs                   ← Helpers de console (leitura, formatação)
    ├── Menus.cs                ← Menus com switch/case
    ├── SelecaoCrud.cs          ← CRUD de seleções
    ├── EstadioCrud.cs          ← CRUD de estádios
    ├── JogoCrud.cs             ← CRUD de jogos + registrar placar
    ├── Classificacao.cs        ← Cálculo da tabela + melhores terceiros
    ├── MataMata.cs             ← Geração e avanço das fases eliminatórias
    └── CsvManager.cs           ← Leitura e gravação de todos os CSVs
```

---

## Funcionalidades implementadas

| # | Funcionalidade | Arquivo |
|---|---|---|
| 1 | CRUD de seleções | SelecaoCrud.cs |
| 2 | CRUD de estádios | EstadioCrud.cs |
| 3 | CRUD de jogos | JogoCrud.cs |
| 4 | Registrar placar + pênaltis | JogoCrud.cs |
| 5 | Tabela de classificação por grupo | Classificacao.cs |
| 6 | Melhores 8 terceiros colocados | Classificacao.cs |
| 7 | Geração da Fase de 32 (mata-mata) | MataMata.cs |
| 8 | Avanço de fases (oitavas → final) | MataMata.cs |
| 9 | Disputa de 3° lugar | MataMata.cs |
| 10 | Mostrar chave do mata-mata e campeão | MataMata.cs |
| 11 | Salvar/carregar CSV | CsvManager.cs |
| 12 | Relatório final em CSV | CsvManager.cs |

---

## Conceitos de programação estruturada utilizados

- **struct**: `Selecao`, `Estadio`, `Jogo`
- **vetores**: `selecoes[]`, `estadios[]`, `jogos[]`
- **matriz**: `int[,] tabela` — 48 linhas × 8 colunas de estatísticas
- **funções estáticas**: todas as operações separadas por responsabilidade
- **switch/case**: todos os menus
- **validações**: grupo A-L, limite 4 seleções/grupo, ID único, placar não-negativo, etc.
- **CSV**: leitura e gravação com `File.ReadAllLines` / `File.WriteAllText`

---

## Fluxo sugerido para a apresentação

1. Carregar dados (opção 12) — estádios e seleções já vêm pré-carregados
2. Cadastrar alguns jogos da fase de grupos (opção 3 → 1)
3. Registrar placares (opção 4)
4. Gerar tabela dos grupos (opção 5)
5. Ver melhores terceiros (opção 6)
6. Gerar mata-mata — Fase de 32 (opção 7)
7. Registrar placares dos jogos do mata-mata (opção 4)
8. Avançar fases até a Final (opção 8)
9. Ver chave e campeão (opção 9)
10. Gerar relatório final (opção 10 → 1)
11. Salvar tudo (opção 11)

---

## Arquivos CSV obrigatórios

| Arquivo | Conteúdo |
|---|---|
| selecoes.csv | 48 seleções com grupo |
| estadios.csv | 16 estádios da Copa 2026 |
| jogos.csv | Todos os jogos com placar e fase |
| classificacao.csv | Tabela calculada dos grupos |
| mata_mata.csv | Jogos eliminatórios |
| relatorio_final.csv | Resumo completo da Copa |

---

## Critérios de desempate (fase de grupos)

1. Maior número de pontos
2. Maior número de vitórias
3. Maior saldo de gols
4. Maior número de gols marcados
5. Ordem alfabética do nome da seleção
