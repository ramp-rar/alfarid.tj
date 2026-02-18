# Alfarid architecture (MVP foundation)

## Product components
- `Alfarid.Teacher` (WPF GUI)
- `Alfarid.Student` (WPF GUI)
- `Alfarid.ControlPlane` (gRPC contract + Teacher host service)
- `Alfarid.Shared` (shared networking utilities like discovery)
- `Alfarid.Agent` (reserved for future hard-control service)

## Realtime model
- Topology: **Peer-to-Host** (Teacher is session host).
- Discovery: UDP broadcast (`ALFARID_TEACHER_DISCOVERY_V1` over port `49555`).
- Control plane: gRPC service `TeacherHub` on port `5055`.

## Current MVP foundation implemented
1. Teacher host starts gRPC service.
2. Teacher host broadcasts discovery packets to LAN.
3. Student listens for discovery, registers, and sends heartbeat.
4. Teacher UI has starter classroom grid layout for student cards.

## Next milestones
- Hook Teacher UI to live list from `TeacherHubService`.
- Add event stream (`SubscribeEvents`) with register/hand raised events.
- Add command delivery routing from Teacher to Student.
