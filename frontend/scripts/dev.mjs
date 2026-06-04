import { spawn } from 'node:child_process'
import { realpathSync } from 'node:fs'
import { dirname, join } from 'node:path'
import { fileURLToPath } from 'node:url'

const root = realpathSync(join(dirname(fileURLToPath(import.meta.url)), '..'))
const vite = process.platform === 'win32' ? 'npx.cmd' : 'npx'

const child = spawn(vite, ['vite'], { cwd: root, stdio: 'inherit', shell: true })
child.on('exit', (code) => process.exit(code ?? 1))
