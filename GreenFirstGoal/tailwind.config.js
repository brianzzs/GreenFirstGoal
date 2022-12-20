/** @type {import('tailwindcss').Config} */
module.exports = {
purge: {
    enabled: true,
    content: [
        './Views/**/*.cshtml'
    ]
},
  theme: {
    extend: {},
  },
  plugins: [],
}
