# Demo OpenAI e OpenCV
Questa demo è composta di due applicazioni per il desktop di Windows (.NET 8, WPF). Inoltre, qui trovi la [presentazione PDF](Presentazione.pdf) dell'evento.

## openai-demo
È un semplice client che usa la [Chat Completions API](https://platform.openai.com/docs/guides/text-generation/chat-completions-api) di OpenAI e il modello multimodale `GPT-4o` per estrarre informazioni da un'immagine. È necessario fornire una [API Key](https://platform.openai.com/api-keys) nel file [appsettings.json](openai-demo/appsettings.json) o via user secrets.

## opencv-demo
È un frontend per OpenCV che permette di sperimentare con le funzionalità Threshold, Adaptive Threshold, Morph, Blob Detection e Canny Edge detection.
Inoltre, permette di applicare delle label alle immagini ed eseguire un algoritmo genetico ([GeneticSharp](https://www.nuget.org/packages/GeneticSharp)) per trovare i valori migliori.
