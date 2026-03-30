export type EventStatus = "DRAFT" | "PUBLISHED" | string;

export type EventListItem = {
  id: string;
  name: string;
  eventDate: string;
  place: string;
  status: EventStatus;
};

export type CreateZoneInput = {
  name: string;
  price: number;
  capacity: number;
};

export type CreateEventInput = {
  name: string;
  eventDate: string;
  place: string;
  zones: CreateZoneInput[];
};

// Backend DTO contract for POST /events
export type CreateEventRequestDto = {
  Name: string;
  EventDate: string;
  Place: string;
  Zones: Array<{
    Name: string;
    Price: number;
    Capacity: number;
  }>;
};
