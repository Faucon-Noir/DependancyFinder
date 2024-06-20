# DependencyFinder.Tool

## Minimal to run

Clone project, open it, restore its dependencies then run this:

`dotnet run --project .\DependancyFinder.Tool\ --input Your/Input/Path`

## Options

| parameter          | type   | definition                                                                                                                                                            | default     |
| ------------------ | ------ | --------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------- |
| `-i` or `--input`  | `path` | [REQUIRED] Input path for the file or folder to analyze                                                                                                               | `none`      |
| `-o` or `--output` | `path` | [OPTIONNAL] Output path for the result file. Created if not exist but specified. Can write on an other drive only if the drive exist, else, it will stop the programm | `./OutPut/` |
