{{/*
Dynamic default name based on the parent key or use nameOverride if provided.
*/}}
{{- define "fileserver.resourcename" -}}
{{- if .nameOverride -}}
{{- printf "%s" .nameOverride -}}
{{- else -}}
{{- printf "%s" .defaultName -}}
{{- end -}}
{{- end }}

{{/*
Generate standard labels
*/}}
{{- define "fileserver.labels" -}}
helm.sh/chart: {{ printf "%s-%s" .Chart.Name .Chart.Version }}
app.kubernetes.io/name: {{ .resourceName }}
app.kubernetes.io/instance: {{ .Release.Name }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Generate standard selector labels
*/}}
{{- define "fileserver.selectorLabels" -}}
app.kubernetes.io/name: {{ .resourceName }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end -}}