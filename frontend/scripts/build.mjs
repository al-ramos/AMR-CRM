import { spawnSync } from 'node:child_process'
import { realpathSync } from 'node:fs'
import { dirname, join } from 'node:path'
import { fileURLToPath } from 'node:url'

const root = realpathSync(join(dirname(fileURLToPath(import.meta.url)), '..'))
const npx = process.platform === 'win32' ? 'npx.cmd' : 'npx'
const opts = { cwd: root, stdio: 'inherit', shell: true }

const tsc = spawnSync(npx, ['tsc', '-b'], opts)
if (tsc.status !== 0) process.exit(tsc.status ?? 1)

const vite = spawnSync(npx, ['vite', 'build'], opts)
process.exit(vite.status ?? 1)
