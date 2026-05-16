# Meeting 01 — Kickoff Agenda

> Zweck: gemeinsame Engine-/Workflow-/Scope-Entscheidungen, damit Phase 4+ (Schießmechanik, Zombies) nicht in Wegwerfarbeit endet.
> Begleitmaterial: [../README.md](../README.md) · [../Architecture.md](../Architecture.md) · [implementation-plan.md](implementation-plan.md)

## Vor dem Treffen lesen (5 Min)
1. [README.md](../README.md) — Status, Quick Start
2. [Architecture.md](../Architecture.md) — Engine-Entscheidung, Comfort-Targets
3. [implementation-plan.md](implementation-plan.md) — Backlog + Design Discussions D1–D4

## Was bereits steht (kein Diskussionsbedarf)
- ✅ Engine: **Unity 6000.4.7f1** + C# + URP
- ✅ XR-Stack: OpenXR + XR Interaction Toolkit 3.0.7
- ✅ Ziel-Plattform: Meta Quest (PCVR Link)
- ✅ Hello-World läuft (mit + ohne Headset via Device Simulator)
- ✅ Repo-Konventionen: Conventional Commits, Branch `main`

## Live-Demo (10 Min)
1. `Open Unity Editor` ausführen → HelloVR-Szene zeigen
2. Mit XR Device Simulator vorführen (keine Brille nötig): umsehen, Würfel greifen, teleportieren
3. Console: Grab-Logs zeigen
4. Falls Headset da: Quest Link + Play

## Entscheidungspunkte (Hauptteil)

### D1 — Locomotion: Teleport, Smooth oder beides?
- **Teleport**: motion-sickness-arm, weniger immersiv, für Zombie-Shooter eher ungewöhnlich
- **Smooth**: shooter-typisch, aber 30–50 % der Spieler werden seekrank
- **Beides (Comfort-Menü)**: empfohlen, kostet ~1 Tag Mehraufwand
- **Frage**: Wer im Team verträgt was? Wollen wir auf „shooter-feel" oder „accessible" optimieren?
- **Empfehlung**: Teleport als Default, Smooth als Opt-in im Pause-Menü

### D2 — Build-Pipeline: PCVR-only oder Standalone APK?
- **PCVR-only**: einfach, performance-tolerant, Quest hängt via Link am PC
- **Standalone**: APK auf Quest installierbar, kein PC nötig, aber harte Performance-Limits (Mobile-GPU)
- **Frage**: Wollen wir spielbar ohne PC-Setup?
- **Empfehlung**: PCVR-only bis Phase 5, dann evaluieren

### D3 — Hand-Darstellung
- Controller-Modelle (Standard) vs stilisierte Hände vs Hand-Tracking
- **Empfehlung**: Controller bleiben, bis erste Waffen funktionieren

### D4 — Git-Workflow
- Direkt auf `main` committen (drei Leute, kein Reviewer) vs Feature-Branches + PRs
- **Empfehlung**: Feature-Branches + PRs ab jetzt — auch ohne formales Review verhindert es Merge-Konflikte und schafft Diff-Übersicht

## Rollen / Verantwortlichkeiten
- Wer kümmert sich um was?
  - Gameplay-Code (Waffen, Zombies)
  - Art / Assets (Modelle, Texturen, Sound)
  - Level-Design (Arena-Layout)
  - Tooling / CI / Build
- AI-Agents übernehmen den Großteil des Codes — wer reviewt deren Output?

## Scope-Frage: Was ist das „Minimum Viable Prototype"?
Vorschlag in vier Mini-Milestones:
1. **MVP-A**: Pistole am Gürtel greifen, Raycast-Schuss, ein statischer Zombie umfällt → 1 Wochenende
2. **MVP-B**: Zombie läuft auf Spieler zu (NavMesh), kann den Spieler angreifen → 1 Wochenende
3. **MVP-C**: Wave-Spawner, einfache Score-Anzeige → 1 Wochenende
4. **MVP-D**: Polish — Sound, Particles, Menü → 1 Wochenende

→ **Frage**: Tempo realistisch? Welche MVP-Stufe ist „showcase-würdig"?

## Tooling-Setup für alle
- [ ] Alle haben Unity 6000.4.7f1 installiert?
- [ ] Alle haben Meta Quest Link funktionierend (falls Headset vorhanden)?
- [ ] Alle haben GitHub-Zugang zum Repo `ducktalez/VRtasia`?
- [ ] Alle nutzen denselben C#-IDE-Stil (Rider / VS / VSCode — egal, aber einheitliches Editor-Format)?
- [ ] AI-Agent-Setup (GitHub Copilot, Claude Code, o.ä.) — wer nutzt was?

## Nach dem Treffen
- Entscheidungen in [Architecture.md](../Architecture.md) eintragen
- D1–D4 in [implementation-plan.md](implementation-plan.md) von `TBD` auf entschieden setzen
- Rollen + MVP-Plan in `implementation-plan.md` festhalten
- Issues auf GitHub erstellen für MVP-A-Tasks

